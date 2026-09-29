namespace S13Audio;

public interface IS13Controllable
{
	string name { get; }

	void Play();

	void Play(float duration);

	void PlayDelayed(float delayTime);

	void PlayDelayed(float delayTime, float duration);

	void Stop();

	void Stop(bool ignoreFades);

	void StopDelayed(float delayTime);

	void StopDelayed(float delayTime, bool ignoreFades);

	void Shutdown();

	void Pause();

	void Resume();
}
